import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { SpecializationRow } from '../../../../types';

import { ChevronButton } from '@/components/ChevronButton';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import { ButtonsWrapper, CellTypography, CellWrapper } from './styles';

export function SpecializationCell({
	row,
	onRowChange,
}: RenderCellProps<SpecializationRow, unknown>) {
	const { showSkeleton, isPrimary, name, isExpanded, hardSkills } = row;

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
					<ChevronButton isActive={isExpanded} onClick={handleExpandClick} />
				)}
			</ButtonsWrapper>
		</CellWrapper>
	);
}
