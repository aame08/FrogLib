create database froglib;
use froglib;

-- Список «Авторы»
create table Authors (
    id_author int auto_increment,
    surname_author varchar(100) not null,
    name_author varchar(100) not null,
    patronymic_author varchar(100),
    primary key (id_author)
);

-- Справочник «Категории»
create table Categories (
    id_category int auto_increment,
    name_category varchar(100) not null unique,
    primary key (id_category)
);

-- Справочник «Издатели»
create table Publishers (
    id_publisher int auto_increment,
    name_publisher varchar(100) not null unique,
    primary key (id_publisher)
);

-- Справочник «Книги»
create table Books (
    id_book int auto_increment,
    isbn_13 varchar(13) not null unique,
    id_publisher int not null,
    id_category int not null,
    image_url text not null,
    title_book varchar(255) not null,
    description_book text not null,
    year_publication year not null,
    page_count int not null,
    language_book varchar(10) not null,
    added_date date not null,
    status_book enum('', 'Новинка', 'Бестселлер'),
    primary key (id_book),
    foreign key (id_publisher) references Publishers(id_publisher),
    foreign key (id_category) references Categories(id_category)
);

-- Связь между книгами и авторами
create table BookAuthors (
    id_author int not null,
    id_book int not null,
    primary key (id_author, id_book),
    foreign key (id_author) references Authors(id_author),
    foreign key (id_book) references Books(id_book)
);

-- Список «Пользователи»
create table Users  (
    id_user int auto_increment,
    name_role varchar(50) not null,
    name_user varchar(255) not null,
    login_user varchar(100) not null unique,
    password_hash varchar(60) not null,
    profile_image_url text,
    registration_date date not null,
    status_user enum('Активен', 'Заблокирован') not null,
    primary key (id_user)
);

-- Список «Нарушения»
create table Violations (
    id_violation int auto_increment,
    id_user int not null,
    category_violation enum('Спам', 'Оскорбления', 'Мошенничество', 'Призывы к насилию', 'Другое') not null,
    description_violation text not null,
    date_violation date not null,
    primary key (id_violation),
    foreign key (id_user) references Users(id_user)
);

-- Справочник «Тип списка»
create table ListTypes (
    id_list_type int auto_increment,
    name_list varchar(100) not null unique,
    primary key (id_list_type)
);

-- Список «Книги пользователя»
create table UserBooks (
    id_list_type int not null,
    id_user int not null,
    id_book int not null,
    added_date date not null,
    primary key (id_list_type, id_user, id_book),
    foreign key (id_list_type) references ListTypes(id_list_type),
    foreign key (id_user) references Users(id_user),
    foreign key (id_book) references Books(id_book)
);

-- Список «Подборки»
create table Collections (
    id_collection int auto_increment,
    id_user int not null,
    title_collection varchar(255) not null,
    description_collection text,
    created_date date not null,
    status_collection enum('На рассмотрении', 'Обнаружено нарушение', 'Одобрено', 'Отказано') not null,
    primary key (id_collection),
    foreign key (id_user) references Users(id_user)
);

-- Связь между книгами и подборками
create table BookCollections (
	id_collection int not null,
    id_book int not null,
    primary key (id_collection, id_book),
    foreign key (id_collection) references Collections(id_collection),
    foreign key (id_book) references Books(id_book)
);

-- Список «Понравившиеся подборки»
create table LikedCollections (
    id_collection int not null,
    id_user int not null,
    liked_date date not null,
    primary key (id_collection, id_user),
    foreign key (id_collection) references Collections(id_collection),
    foreign key (id_user) references Users(id_user)
);

-- Список «Комментарии»
create table Comments (
    id_comment int auto_increment,
    id_user int not null,
    id_entity int not null,
    type_entity enum('Рецензия', 'Подборка') not null,
    text_comment text not null,
    date_comment date not null,
    parent_comment_id int,
    status_comment enum('Обнаружено нарушение', 'Новое', 'Просмотрено') not null,
    primary key (id_comment),
    foreign key (id_user) references Users(id_user),
    foreign key (parent_comment_id) references Comments(id_comment)
);

-- Список «Оценки книг»
create table BookRatings (
    id_rating int auto_increment,
    id_user int not null,
    id_book int not null,
    rating int not null,
    primary key (id_rating),
    foreign key (id_user) references Users(id_user),
    foreign key (id_book) references Books(id_book)
);

-- Список «Оценки рецензий и подборок»
create table EntityRatings (
    id_rating int auto_increment,
    id_user int not null,
    id_entity int not null,
    type_entity enum('Рецензия', 'Подборка') not null,
    rating tinyint not null,
    primary key (id_rating),
    foreign key (id_user) references Users(id_user)
);

-- Список «Рецензии»
create table Reviews (
    id_review int auto_increment,
    id_user int not null,
    id_book int not null,
    title_review varchar(255) not null,
    text_review text not null,
    created_date date not null,
    status_review enum('На рассмотрении', 'Обнаружено нарушение', 'Одобрено', 'Отказано') not null,
    primary key (id_review),
    foreign key (id_user) references Users(id_user),
    foreign key (id_book) references Books(id_book)
);

-- Список «История просмотра»
create table ViewHistory (
    id_history int auto_increment,
    id_user int not null,
    id_entity int not null,
    type_entity enum('Книга', 'Рецензия', 'Подборка') not null,
    view_date date not null,
    primary key (id_history),
    foreign key (id_user) references Users(id_user)
);