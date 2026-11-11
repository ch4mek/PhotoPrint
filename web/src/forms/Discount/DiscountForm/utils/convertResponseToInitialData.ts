import { Discount } from '@/types/discounts';

export const convertResponseToInitialData = (discountData: Discount) => ({
	name: discountData.name,
	discountCategory: {
		id: discountData.discountCategoryId,
		name: discountData.discountCategoryName,
	},
	companyType: discountData?.companyType || '',
	website: discountData?.website || '',
	phoneNumber: discountData?.phoneNumber || '',
	location: discountData?.location || '',
	discountPercent:
		discountData.discountPercent || discountData.discountPercent === 0
			? String(discountData.discountPercent)
			: '',
	discountPercentTo:
		discountData.discountPercentTo || discountData.discountPercentTo === 0
			? String(discountData.discountPercentTo)
			: '',
	tags: discountData.discountTags.map(({ tagId, name }) => ({
		name,
		id: tagId,
	})),
	cities: discountData.discountCities.map(({ cityId, name }) => ({
		id: cityId,
		name,
	})),
	mediaLinks: discountData.discountReferences.map(
		({ socialMediaUrl }) => socialMediaUrl
	),
	imageChangeMarker: discountData.imageFileId,
});
