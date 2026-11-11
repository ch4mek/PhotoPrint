import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderCellProps } from 'react-data-grid';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import { SpecializationHardSkillRow } from '@/pages/TeamBoardUser/types';

import { CellWrapper } from './styles';

export function HardSkillCell({
	row,
}: RenderCellProps<SpecializationHardSkillRow, unknown>) {
	const { name, isActive } = row;

	return (
		<CellWrapper isActive={isActive}>
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={name}
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				useIfTextCut
			>
				<Typography variant="m-400">{name}</Typography>
			</Tooltip>
		</CellWrapper>
	);
}
