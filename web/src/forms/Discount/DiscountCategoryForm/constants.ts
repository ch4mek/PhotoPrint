import { DiscountCategoryFormValues } from '@/types/discountCategories';

export const DISCOUNT_CATEGORY_FIELD_TITLE = 'Discount category';
export const FORM_NAME = 'Discount category form';

export const fieldNames = {
	tags: 'discountCategoryTags',
	tagName: 'tagName',
	categoryName: 'name',
};

export const initialFormValues: DiscountCategoryFormValues = {
	discountCategoryTags: [],
	name: '',
	tagName: '',
};

export const discountCategoryFieldData = {
	categoryName: {
		name: fieldNames.categoryName,
		label: 'Discount category name',
	},
	newTagName: {
		name: fieldNames.tagName,
		label: 'New tag',
	},
	tags: {
		valueField: fieldNames.tags,
		label: 'Existing tags',
	},
};
