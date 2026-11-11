import styled from '@emotion/styled';

export const PopUpWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const DescriptionWrapper = styled('span')(() => ({}));
