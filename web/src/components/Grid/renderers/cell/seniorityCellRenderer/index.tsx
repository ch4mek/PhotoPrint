import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { SeniorityCellRenderer } from '@/components/Grid/renderers/cell/seniorityCellRenderer/types';
import { zIndexes } from '@/constants/zIndexes';

import { CellWrapper } from './styles';

export const seniorityCellRenderer = <
	T extends GridCellDefaultProps & SeniorityCellRenderer<U>,
	U
>(
	{ row: { snapshots }, column: { key } }: RenderCellProps<T>,
	fieldToDisplay: keyof U,
	isMarkAsPast: boolean
) => {
	const info = (snapshots[key]?.[fieldToDisplay] ?? '').toString();

	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={info}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper isMarkAsPast={isMarkAsPast}>
				<Typography variant="m-600">{info}</Typography>
			</CellWrapper>
		</Tooltip>
	);
};
