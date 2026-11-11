import styled from '@emotion/styled';

export const SocialMediaItemWrapper = styled('a')(({ theme }) => ({
	color: theme.palette['graphics-primary-2'],

	display: 'flex',
	alignItems: 'center',
}));

export const SocialMediaItemsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,

	marginTop: 'auto',
	height: theme.spaces.m,
}));
