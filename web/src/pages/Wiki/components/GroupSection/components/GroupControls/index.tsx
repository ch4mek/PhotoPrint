import { Button } from '@mms/mms-ui-library/components/Button';
import { SearchInput } from '@mms/mms-ui-library/components/SearchInput';

import { DEFAULT_SEARCH_FIELD_PLACEHOLDER } from '@/constants/controlsTexts';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { groupControlsButtons } from '@/pages/Wiki/components/GroupSection/components/GroupControls/constants';
import {
	GroupControlsWrapper,
	SearchInputWrapper,
} from '@/pages/Wiki/components/GroupSection/components/GroupControls/styles';
import { GroupControlsProps } from '@/pages/Wiki/components/GroupSection/components/GroupControls/types';
import { UserClaims } from '@/types/claims';
import { SegmentTypes } from '@/types/wiki';

const { move, addGroup, save } = groupControlsButtons;

export function GroupControls({
	handleSearchChange,
	searchValue,
	handleAddGroupClick,
	isMoveModeEnable,
	handleModeButtonClick,
	handleSearchReset,
	isButtonsDisabled,
}: GroupControlsProps) {
	const { claims } = useAuth();

	return (
		<GroupControlsWrapper>
			<>
				{checkIsHasClaims([UserClaims.ManageGroup], claims) && (
					<Button
						isFullWidth={false}
						onClick={handleAddGroupClick(SegmentTypes.Group)}
						disabled={isButtonsDisabled || isMoveModeEnable}
					>
						{addGroup}
					</Button>
				)}

				{checkIsHasClaims([UserClaims.ManageOrder], claims) && (
					<Button
						isFullWidth={false}
						color="secondary"
						onClick={handleModeButtonClick}
						disabled={isButtonsDisabled}
					>
						{isMoveModeEnable ? save : move}
					</Button>
				)}
			</>
			<SearchInputWrapper>
				<SearchInput
					placeholder={DEFAULT_SEARCH_FIELD_PLACEHOLDER}
					value={searchValue}
					onChange={handleSearchChange}
					size="fullWidth"
					disabled={isMoveModeEnable}
					onDeleteIconClick={handleSearchReset}
					showToolTipIfCut={false}
				/>
			</SearchInputWrapper>
		</GroupControlsWrapper>
	);
}
