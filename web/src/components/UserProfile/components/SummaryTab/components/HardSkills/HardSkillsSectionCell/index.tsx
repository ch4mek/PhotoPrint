import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import { CellTypography, CellWrapper } from './styles';
import { HardSkillsSectionCellProps } from './types';

export function HardSkillsSectionCell({
	content,
	isPrimary,
}: HardSkillsSectionCellProps) {
	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={content}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper isPrimary={isPrimary}>
				<CellTypography variant="m-400" isPrimary={isPrimary}>
					{content}
				</CellTypography>
			</CellWrapper>
		</Tooltip>
	);
}
