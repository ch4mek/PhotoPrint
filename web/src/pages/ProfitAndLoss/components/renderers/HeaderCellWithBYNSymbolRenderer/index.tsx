import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderHeaderCellProps } from 'react-data-grid';

import { CurrencySymbol } from '@/components/CurrencySymbol';
import { HeaderCellRenderer } from '@/components/Grid/renderers/header/headerCellRenderer';
import { GridCellDefaultProps } from '@/components/Grid/renderers/types';
import { Currency } from '@/constants/currency';

import { CurrencySymbolTypography } from './styles';

export const HeaderCellWithBYNSymbolRenderer = <
	TRow extends GridCellDefaultProps,
	TSummaryRow = unknown
>(
	props: RenderHeaderCellProps<TRow, TSummaryRow>
) =>
	HeaderCellRenderer({
		...props,
		isContentAlignedToEnd: true,
		nameContent: (
			<Typography variant="s-600">
				{props.column.name},
				<CurrencySymbolTypography variant="m-400">
					<CurrencySymbol currency={Currency.BYN} />
				</CurrencySymbolTypography>
			</Typography>
		),
	});
