import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { useSpecializationTableContext } from '../../context/specializationTableContext';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import { SpecializationRow } from '@/pages/TeamBoardUser/types';

import {
	ButtonsWrapper,
	CellTypography,
	CellWrapper,
	PrimarySpecializationChevronButton,
} from './styles';

export function SpecializationCell({
	row,
	onRowChange,
}: RenderCellProps<SpecializationRow, unknown>) {
	const { showSkeleton, isPrimary, name, isExpanded, hardSkills } = row;

	const { contextMenuOption } = useSpecializationTableContext();

	const handleExpandClick = () => {
		onRowChange({
			...row,
			isExpanded: !isExpanded,
		});
	};

	if (showSkeleton) {
		return (
			<CellWrapper>
				<Skeleton animation="wave" width="100%" />
			</CellWrapper>
		);
	}

	const hasSkills = hardSkills.length > 0;

	return (
		<CellWrapper>
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={name}
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				useIfTextCut
			>
				<CellTypography variant="m-400" isPrimary={isPrimary}>
					{name}
				</CellTypography>
			</Tooltip>
			<ButtonsWrapper>
				{hasSkills && (
					<PrimarySpecializationChevronButton
						isPrimary={isPrimary}
						isActive={isExpanded}
						onClick={handleExpandClick}
					/>
				)}
				{!isPrimary && (
					<PickerButton
						icon={<OptionDotsIcon />}
						options={contextMenuOption}
						item={row}
						popoverPosition="left-start"
					/>
				)}
			</ButtonsWrapper>
		</CellWrapper>
	);
}
