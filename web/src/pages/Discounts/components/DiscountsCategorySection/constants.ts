export const SECTION_FIELD_TITLE = 'Discount category';

export const ALL_CATEGORIES_TITLE = 'All categories';

export const allCategoriesItem = {
	id: ALL_CATEGORIES_TITLE,
	name: ALL_CATEGORIES_TITLE,
	orderWeight: -1,
	discountCategoryTags: [],
};

export const getUpdateOrderTitle = (type: string) => `The ${type} order`;
