import styled from '@emotion/styled';

export const DecorationImage = styled('picture')<{
	hideBelow?: number;
}>(({ hideBelow }) => ({
	display: 'flex',
	alignItems: 'flex-end',

	'& img': {
		height: 'auto',
		maxWidth: '100%',
		objectFit: 'contain',
	},

	...(hideBelow && {
		[`@media (max-width: ${hideBelow}px)`]: {
			display: 'none',
		},
	}),
}));
