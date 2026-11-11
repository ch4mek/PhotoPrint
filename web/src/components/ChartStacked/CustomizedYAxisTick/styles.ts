import styled from '@emotion/styled';

export const Text = styled('text')(({ theme }) => ({
	fontSize: theme.typography.pxToRem(12),
	color: theme.palette['text-gray'],
	fill: theme.palette['text-gray'],
	fontWeight: 500,
}));
