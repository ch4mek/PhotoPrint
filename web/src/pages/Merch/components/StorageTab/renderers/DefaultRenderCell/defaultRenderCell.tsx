import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { cellWithTooltipRenderer } from '@/components/Grid';
import { RESIZABLE_ROW_LINE_HEIGHT } from '@/constants/commonStyles';
import { getMaxVerticalLineCount } from '@/pages/Merch/utils/getMaxVerticalLineCount';
import { DefaultCellRendererProps } from '@/pages/Payload/types';

import { CellWrapper } from './styles';

export const defaultCellRenderer = ({
	props,
	options,
}: DefaultCellRendererProps<MerchStorageGridRow>) => {
	const {
		row,
		column: { key },
	} = props;

	const theme = useTheme();

	const { withTooltip, isResizable } = options;
	const { itemsRowHeight, expanded, sizeAmounts } = row;
	const info = row[key as keyof MerchStorageGridRow] as string;
	const tooltip = cellWithTooltipRenderer(props, {
		isCopyEnabled: false,
		textCutOrientation: 'vertical',
	});

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	if (withTooltip) {
		const maxVerticalLineCount = getMaxVerticalLineCount(itemsRowHeight || 0);

		return (
			<CellWrapper
				isExpanded={expanded}
				maxVerticalLineCount={isResizable ? maxVerticalLineCount : null}
				childHeight={`${maxVerticalLineCount * RESIZABLE_ROW_LINE_HEIGHT}px`}
				childWidth="auto"
				color={cellColor}
			>
				{tooltip}
			</CellWrapper>
		);
	}

	return (
		<CellWrapper isExpanded={expanded} color={cellColor}>
			{info}
		</CellWrapper>
	);
};
