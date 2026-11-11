export interface Discount {
	id: number;
	discountCategoryId: number;
	discountCategoryName: string;
	name: string;
	imageFileId: string | null;
	discountPercent: number;
	discountPercentTo: number | null;
	companyType: string | null;
	website: string | null;
	phoneNumber: string | null;
	location: string | null;
	text: string;
	isActive: boolean;
	orderWeight: number;
	discountReferences: DiscountReference[];
	discountTags: FullDiscountTag[];
	discountCities: DiscountCity[];
}

export type DiscountAddData = Omit<
	Discount,
	| 'id'
	| 'discountCategoryName'
	| 'orderWeight'
	| 'discountReferences'
	| 'discountTags'
	| 'discountCities'
	| 'isActive'
> & {
	discountTagIds: number[];
	discountCityIds: number[];
	discountSocialMediaUrls: string[];
};

export type DiscountSubmitData = DiscountAddData;

export type DiscountUpdateData = Omit<
	Discount,
	| 'discountCategoryName'
	| 'orderWeight'
	| 'discountReferences'
	| 'discountTags'
	| 'discountCities'
> & {
	discountTagIds: number[];
	discountCityIds: number[];
	discountSocialMediaUrls: string[];
};

interface DiscountReference {
	id: number;
	discountId: number;
	socialMediaUrl: string;
}

interface FullDiscountTag {
	id: number;
	name: string;
	discountId: number;
	tagId: number;
}

export interface DiscountTag {
	id: number;
	name: string;
	orderWeight: number;
}

export interface DiscountCity {
	cityId: number;
	name: string;
}

export enum DiscountSegmentTypes {
	Discount = 'Discount',
	Category = 'Category',
}

interface OptionItem {
	id: number;
	name: string;
}

export interface DiscountFormValues {
	name: string;
	discountCategory: OptionItem | null;
	companyType: string;
	website: string;
	phoneNumber: string;
	location: string;
	discountPercent: number | string;
	discountPercentTo: number | string;
	tags: OptionItem[];
	cities: OptionItem[];
	mediaLinks: string[];
	imageChangeMarker: string | null;
}

export interface DiscountOrder {
	discountId: number;
	orderWeight: number;
}

export interface DiscountOrderData {
	discountOrders: DiscountOrder[];
}
