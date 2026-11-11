import styled from '@emotion/styled';
import { IMaskInput } from 'react-imask';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';
import { ThemeMode } from '@/theme';

import { expensesGridZIndexes } from './constants';
import { ExpenseCellInteractiveAttrs } from './types';

const EXPENSE_CELL_ACTION_BUTTON_SIZE = 32;

export const ExpenseCellInteractive = styled(
	'span'
)<ExpenseCellInteractiveAttrs>(
	({ theme, backgroundColor, isDisabled, minHeight, isClickable }) => ({
		backgroundColor,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'flex-end',
		borderBottom: `1px solid ${theme.palette['graphics-divider-2']}`,
		cursor: isDisabled || !isClickable ? 'default' : 'pointer',
		pointerEvents: isDisabled ? 'none' : 'auto',
		opacity: isDisabled ? HALF_OPACITY : DEFAULT_OPACITY,
		minHeight,
	})
);

export const ExpenseCellWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	height: '100%',
	position: 'relative',
	overflow: 'visible',
	boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-2']}`,
}));

export const ExpenseAmountInput = styled(IMaskInput)(({ theme }) => ({
	backgroundColor: 'transparent',
	width: '102%',
	height: '100%',
	color:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['text-black']
			: theme.palette['text-white'],
	textAlign: 'right',
	paddingRight: theme.spaces.s,
	marginRight: 1,
	'&:focus': {
		outline: 'none',
		boxShadow: `inset 0 0 0 2px ${theme.palette['graphics-accent']}`,
		zIndex: expensesGridZIndexes.input,
	},
	borderRadius: BorderRadiuses.xs,
}));

export const ExpenseAmountInputWrapper = styled.div`
	display: flex;
	flex-direction: column;
	height: 100%;
	justify-content: center;
	align-items: center;
	position: relative;
	background: transparent;
	width: 100%;
`;

export const ExpenseCellActions = styled('div')(({ theme }) => ({
	position: 'absolute',
	display: 'flex',
	justifyContent: 'flex-end',
	gap: theme.spaces.xs,
	'& > button': {
		zIndex: expensesGridZIndexes.icon,
		width: EXPENSE_CELL_ACTION_BUTTON_SIZE,
		height: EXPENSE_CELL_ACTION_BUTTON_SIZE,
	},
}));
