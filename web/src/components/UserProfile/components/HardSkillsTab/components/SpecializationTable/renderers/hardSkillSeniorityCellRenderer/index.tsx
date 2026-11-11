import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { SpecializationHardSkillRow } from '../../../../types';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import { CellWrapper } from './styles';

export function hardSkillSeniorityCellRenderer(
	{
		row: { showSkeleton, snapshots, id, isActive },
		column: { key },
	}: RenderCellProps<SpecializationHardSkillRow, unknown>,
	isMarkAsPast?: boolean
) {
	if (showSkeleton) {
		return (
			<CellWrapper>
				<Skeleton animation="wave" width="100%" />
			</CellWrapper>
		);
	}

	const seniority =
		snapshots[key]?.hardSkills.find(({ id: hardSkillId }) => hardSkillId === id)
			?.seniorityLevelName || '';

	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={seniority}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper isMarkAsPast={isMarkAsPast || !isActive}>
				<Typography variant="m-400">{seniority}</Typography>
			</CellWrapper>
		</Tooltip>
	);
}
