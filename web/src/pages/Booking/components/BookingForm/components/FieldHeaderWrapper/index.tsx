import styled from '@emotion/styled';

export const FieldHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',

	svg: {
		marginRight: theme.spaces.xs,
	},
}));
