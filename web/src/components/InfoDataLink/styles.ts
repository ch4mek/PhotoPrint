import styled from '@emotion/styled';

export const LinkWrapper = styled('a')({
	all: 'unset',
	cursor: 'pointer',
	textDecoration: 'none',

	'&:hover': {
		textDecoration: 'underline',
	},
});
