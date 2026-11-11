import styled from '@emotion/styled';

export const TotalCellLegendWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const TotalCellLegendInfoPattern = styled('span')(({ theme }) => ({
	color: theme.palette['text-gray'],
}));
