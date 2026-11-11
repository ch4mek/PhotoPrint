import { DiscountFormValues, DiscountSubmitData } from '@/types/discounts';

export const convertFormValuesToRequestData = (
	values: DiscountFormValues,
	text: string,
	imageFileId: string | null
) =>
	({
		name: values.name,
		imageFileId,
		discountCategoryId: values.discountCategory?.id,
		companyType: values.companyType?.trim() || null,
		website: values.website?.trim() || null,
		phoneNumber: values.phoneNumber?.trim() || null,
		location: values.location?.trim() || null,
		text,
		discountPercent: values.discountPercent,
		discountPercentTo: values.discountPercentTo
			? values.discountPercentTo
			: null,
		discountTagIds: values.tags.map(({ id }) => id),
		discountCityIds: values.cities.map(({ id }) => id),
		discountSocialMediaUrls: values.mediaLinks.filter((item) => item.trim()),
	} as DiscountSubmitData);
