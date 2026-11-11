import { DraggableControlIcon } from '@/components/DraggableControlIcon';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import {
	CategoryItemContent,
	CategoryItemWrapper,
} from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryItem/styles';
import { CategoryItemProps } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryItem/types';
import { UserClaims } from '@/types/claims';

export function CategoryItem({
	item,
	isMoveMode,
	menuOptions,
	isReadMode,
	handleClick,
	isSelectedItem,
}: CategoryItemProps) {
	const { claims } = useAuth();
	const isDisableOptions =
		!isReadMode &&
		menuOptions &&
		checkIsHasClaims(
			[
				UserClaims.ManageDiscountCategory,
				UserClaims.ManageDiscountOrder,
				UserClaims.ManageDiscountCard,
			],
			claims
		);

	return (
		<CategoryItemWrapper
			onClick={handleClick(item.id)}
			isSelected={isSelectedItem}
		>
			<CategoryItemContent>{item.name}</CategoryItemContent>
			{isDisableOptions && (
				<DraggableControlIcon
					menuOptions={menuOptions}
					isMoveMode={!!isMoveMode}
					withIconSpace={false}
					isDisablePickerButton={checkIsHasClaims(
						[UserClaims.ManageDiscountCategory],
						claims
					)}
					item={{
						id: item.id,
						name: item.name,
					}}
				/>
			)}
		</CategoryItemWrapper>
	);
}
