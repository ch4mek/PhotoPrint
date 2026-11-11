import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { CellWrapper } from '../../../StorageTab/renderers/DefaultRenderCell/styles';
import { getCellColorForRow } from '../../../StorageTab/utils/getCellColorForRow';
import { MerchKitsGridRow } from '../../types';

import { cellWithTooltipRenderer } from '@/components/Grid';
import { MerchStatuses } from '@/pages/Merch/constants';
import { DefaultCellRendererProps } from '@/pages/Payload/types';
import { getMaxVerticalLineCount } from '@/pages/Payload/utils/getMaxVerticalLineCount';

export const DefaultCellRenderer = ({
	props,
	options,
}: DefaultCellRendererProps<MerchKitsGridRow>) => {
	const {
		row,
		column: { key },
	} = props;

	const theme = useTheme();

	const { withTooltip, isResizable } = options;
	const { itemRowHeight, expanded, possibleKits, status } = row;
	const info = row[key as keyof MerchKitsGridRow] as string;
	const tooltip = cellWithTooltipRenderer(props, {
		isCopyEnabled: false,
		textCutOrientation: 'vertical',
	});

	const cellColor = useMemo(
		() => getCellColorForRow(possibleKits, theme, expanded),
		[row, theme]
	);

	const isDisabled = status === MerchStatuses.archived;

	if (withTooltip) {
		const maxVerticalLineCount = getMaxVerticalLineCount(itemRowHeight || 0);

		return (
			<CellWrapper
				isExpanded={expanded}
				maxVerticalLineCount={isResizable ? maxVerticalLineCount : null}
				childHeight="auto"
				childWidth="auto"
				color={cellColor}
				disabled={isDisabled}
			>
				{tooltip}
			</CellWrapper>
		);
	}

	return (
		<CellWrapper disabled={isDisabled} isExpanded={expanded} color={cellColor}>
			{info}
		</CellWrapper>
	);
};
