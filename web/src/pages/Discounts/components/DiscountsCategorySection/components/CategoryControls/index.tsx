import { Button } from '@mms/mms-ui-library/components/Button';

import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { categoryControlsButtons } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryControls/constants';
import { CategoryControlsWrapper } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryControls/styles';
import { CategoryControlsProps } from '@/pages/Discounts/components/DiscountsCategorySection/components/CategoryControls/types';
import { UserClaims } from '@/types/claims';

const { addCategory, cancel, move, save } = categoryControlsButtons;

export function CategoryControls({
	isMoveModeEnable,
	handleAddButton,
	handleCancelButtonClick,
	handleModeButtonClick,
	isButtonsDisabled,
	isMoveEnable,
}: CategoryControlsProps) {
	const { claims } = useAuth();

	return (
		<CategoryControlsWrapper>
			{checkIsHasClaims([UserClaims.ManageDiscountCategory], claims) && (
				<Button
					isFullWidth={false}
					onClick={handleAddButton}
					disabled={isButtonsDisabled || isMoveModeEnable}
				>
					{addCategory}
				</Button>
			)}
			{checkIsHasClaims([UserClaims.ManageDiscountOrder], claims) && (
				<Button
					isFullWidth={false}
					color="secondary"
					onClick={handleModeButtonClick}
					disabled={isButtonsDisabled || isMoveEnable}
				>
					{isMoveModeEnable ? save : move}
				</Button>
			)}
			{isMoveModeEnable && (
				<Button
					isFullWidth={false}
					color="secondary"
					onClick={handleCancelButtonClick}
				>
					{cancel}
				</Button>
			)}
		</CategoryControlsWrapper>
	);
}
