import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import { CellWrapper } from './styles';
import { SoftSkillCellProps } from './types';

export function SoftSkillCell({ content }: SoftSkillCellProps) {
	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={content}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper>
				<Typography variant="m-400">{content}</Typography>
			</CellWrapper>
		</Tooltip>
	);
}
