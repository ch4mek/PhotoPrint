import { CategoryTagsList } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/CategoryTagsList';
import { CitiesTagList } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/CitiesTagList';
import { SearchPanel } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/SearchPanel';
import {
	ControlRow,
	DiscountControlsWrapper,
} from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/styles';
import { DiscountControlsProps } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/types';

export function DiscountControls({
	isControlsDisabled,
}: DiscountControlsProps) {
	return (
		<DiscountControlsWrapper>
			<ControlRow reverseWrap>
				<CitiesTagList />
				<SearchPanel isControlsDisabled={isControlsDisabled} />
			</ControlRow>
			<CategoryTagsList />
		</DiscountControlsWrapper>
	);
}
