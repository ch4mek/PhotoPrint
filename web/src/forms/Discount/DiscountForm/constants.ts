import { DiscountFormValues } from '@/types/discounts';

export const DEFAULT_CONTENT_HEIGHT = 490;

export const fieldNames = {
	name: 'name',
	discountCategory: 'discountCategory',
	companyType: 'companyType',
	website: 'website',
	phoneNumber: 'phoneNumber',
	location: 'location',
	discountPercent: 'discountPercent',
	discountPercentTo: 'discountPercentTo',
	tags: 'tags',
	cities: 'cities',
	mediaLinks: 'mediaLinks',
	imageChangeMarker: 'imageChangeMarker',
} as const;

export const initialFormValues: DiscountFormValues = {
	name: '',
	discountCategory: null,
	companyType: '',
	website: '',
	phoneNumber: '',
	location: '',
	discountPercent: '',
	discountPercentTo: '',
	tags: [],
	cities: [],
	mediaLinks: [''],
	imageChangeMarker: null,
};

export const MAX_PERCENT_VALUE_LENGTH = 2;

export const discountFieldData = {
	discountCategory: {
		name: fieldNames.discountCategory,
		label: 'Category',
		valueField: 'name',
		gridArea: fieldNames.discountCategory,
	},
	name: {
		name: fieldNames.name,
		label: 'Discount name',
		gridArea: fieldNames.name,
	},
	discountPercent: {
		name: fieldNames.discountPercent,
		label: 'Percent',
		gridArea: fieldNames.discountPercent,
	},
	discountPercentTo: {
		name: fieldNames.discountPercentTo,
		label: 'Percent to',
		gridArea: fieldNames.discountPercentTo,
	},
	companyType: {
		name: fieldNames.companyType,
		label: 'Company type',
		gridArea: fieldNames.companyType,
	},
	website: {
		name: fieldNames.website,
		label: 'Website',
		gridArea: fieldNames.website,
	},
	phoneNumber: {
		name: fieldNames.phoneNumber,
		label: 'Phone number',
		gridArea: fieldNames.phoneNumber,
	},
	location: {
		name: fieldNames.location,
		label: 'Location',
		gridArea: fieldNames.location,
	},
	cities: {
		valueField: 'name',
		name: fieldNames.cities,
		label: 'Cities',
		gridArea: fieldNames.cities,
	},
	tags: {
		valueField: 'name',
		name: fieldNames.tags,
		label: 'Existing tags',
		gridArea: fieldNames.tags,
	},
	mediaLinks: {
		name: fieldNames.mediaLinks,
		label: 'Social media link',
		gridArea: fieldNames.mediaLinks,
	},
};

export const GENERAL_SECTION_TITLE = 'General info';
export const DESCRIPTION_TITLE = 'Description';
export const PERCENT_RANGE_INVALID_MESSAGE = 'The percent range is invalid';

export const validationFieldsTitles = {
	name: 'discount name',
	discountPercent: 'percent',
	discountPercentTo: 'percent to',
	companyType: 'company type',
	website: 'website',
	phoneNumber: 'phone number',
	mediaLinks: 'media link',
	location: 'location',
};

export const FORM_NAME = 'Discount form';
