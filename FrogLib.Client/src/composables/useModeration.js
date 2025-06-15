import { ref } from 'vue';
import moderService from '@/services/moderService';

export default function useModeration(
  initialData,
  updateStatusFn,
  entityType,
  idField = 'id'
) {
  const localData = ref({ ...initialData });
  const showViolationsForm = ref(false);
  const categoryViolation = ref('Спам');
  const textViolation = ref('');
  const forbiddenWords = ref([]);
  const message = ref('');

  const categories = [
    'Спам',
    'Оскорбления',
    'Мошенничество',
    'Призывы к насилию',
    'Другое',
  ];

  const getForbiddenWords = async () => {
    try {
      const response = await moderService.getForbiddenWords();
      forbiddenWords.value = Array.from(response);
    } catch (error) {
      console.error('Ошибка при загрузке запрещенных слов:', error);
    }
  };

  const highlightForbiddenWords = (text) => {
    if (!text || !forbiddenWords.value.length) return text;

    let result = text;

    forbiddenWords.value.forEach((word) => {
      const escapedWord = word.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
      const regex = new RegExp(
        `(^|\\s|[.,!?;:()\\[\\]{}"'])(${escapedWord})(?=$|\\s|[.,!?;:()\\[\\]{}"'])`,
        'gi'
      );
      result = result.replace(
        regex,
        (match, p1, p2) => `${p1}<span class="forbidden-word">${p2}</span>`
      );
    });

    return result;
  };

  const submitViolation = async (onSuccessCallback) => {
    try {
      if (!textViolation.value) {
        message.value = 'Описание нарушения не может быть пустым.';
        return;
      }

      const violationData = {
        idUser: localData.value.author.id || localData.value.authorId,
        categoryViolation: categoryViolation.value,
        descriptionViolation: textViolation.value,
      };

      if (entityType === 'Комментарий') {
        await moderService.deleteComment(localData.value.id, violationData);
        if (onSuccessCallback) onSuccessCallback();
      } else {
        await updateStatusFn(localData.value[idField], 'Отказано');
        await moderService.addViolation(violationData);
      }

      message.value = '';
    } catch (error) {
      console.error('Ошибка при отправке нарушения:', error);
    }
  };

  return {
    localData,
    showViolationsForm,
    categoryViolation,
    textViolation,
    forbiddenWords,
    message,
    categories,
    getForbiddenWords,
    highlightForbiddenWords,
    submitViolation,
  };
}
