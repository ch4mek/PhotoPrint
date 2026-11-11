import { Loader } from '@/components/Loader';
import { defaultSearchOptions } from '@/constants/search';
import { useDiscountContext } from '@/context/DiscountContext';
import { DiscountControls } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls';
import { DiscountsList } from '@/pages/Discounts/components/DiscountsSection/components/DiscountsList';
import { DiscountsSectionWrapper } from '@/pages/Discounts/components/DiscountsSection/styles';
import { useGetDiscounts } from '@/queries/discounts/useGetDiscounts';

export function DiscountsSection() {
	const { activeCityId, activeTagId, activeCategoryId, debouncedSearchValue } =
		useDiscountContext();

	const { data, isLoading, isFetching } = useGetDiscounts({
		tagId: activeTagId,
		cityId: activeCityId,
		discountCategoryId: activeCategoryId,
		name:
			debouncedSearchValue.length <= defaultSearchOptions.maxCharacters
				? debouncedSearchValue
				: '',
	});

	const isShowLoader = isLoading || isFetching;

	return (
		<DiscountsSectionWrapper>
			<DiscountControls isControlsDisabled={isShowLoader} />
			<Loader isLoading={isShowLoader}>
				<DiscountsList data={data} />
			</Loader>
		</DiscountsSectionWrapper>
	);
}
