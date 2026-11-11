import styled from '@emotion/styled';

export const ExpenseAmountDisplayWrapper = styled('div')(({ theme }) => ({
	padding: `0 ${theme.spaces.s}px`,
	display: 'flex',
	alignItems: 'baseline',
	width: '100%',
	justifyContent: 'flex-end',
}));

export const ExpenseAmountDisplayContent = styled('span')({
	display: 'inline-block',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	textAlign: 'right',
});
