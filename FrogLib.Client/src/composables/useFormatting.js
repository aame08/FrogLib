import { ref } from 'vue';

export function useFormatting(textCollection) {
  const isBold = ref(false);
  const isItalic = ref(false);
  const isUnderline = ref(false);

  const toggleFormatting = (type) => {
    const textarea = document.querySelector('textarea');
    if (!textarea) return;

    const start = textarea.selectionStart;

    let newText = textCollection.value;

    switch (type) {
      case 'bold':
        newText = isBold.value
          ? `${textCollection.value.slice(
              0,
              start
            )}</b>${textCollection.value.slice(start)}`
          : `${textCollection.value.slice(
              0,
              start
            )}<b>${textCollection.value.slice(start)}`;
        isBold.value = !isBold.value;
        break;
      case 'italic':
        newText = isItalic.value
          ? `${textCollection.value.slice(
              0,
              start
            )}</i>${textCollection.value.slice(start)}`
          : `${textCollection.value.slice(
              0,
              start
            )}<i>${textCollection.value.slice(start)}`;
        isItalic.value = !isItalic.value;
        break;
      case 'underline':
        newText = isUnderline.value
          ? `${textCollection.value.slice(
              0,
              start
            )}</u>${textCollection.value.slice(start)}`
          : `${textCollection.value.slice(
              0,
              start
            )}<u>${textCollection.value.slice(start)}`;
        isUnderline.value = !isUnderline.value;
        break;
    }

    textCollection.value = newText;
    textarea.focus();
    textarea.setSelectionRange(start + 3, start + 3);
  };

  return {
    isBold,
    isItalic,
    isUnderline,
    toggleFormatting,
  };
}
