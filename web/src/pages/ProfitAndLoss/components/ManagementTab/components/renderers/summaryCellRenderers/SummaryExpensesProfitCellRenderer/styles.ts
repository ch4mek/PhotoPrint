import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const SUMMARY_SYMBOL_FONT_SIZE = 9;

export const SummarySymbol = styled('span')({
	display: 'inline',
	verticalAlign: 'text-bottom',
	fontSize: SUMMARY_SYMBOL_FONT_SIZE,
});

export const CellContent = styled(Typography)(({ theme }) => ({
	flex: '0 0 auto',
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));

export const SummaryExpensesProfitCell = styled('span')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.l,
	alignItems: 'center',
}));
