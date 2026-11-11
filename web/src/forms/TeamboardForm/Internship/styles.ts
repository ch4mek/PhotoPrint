import styled from '@emotion/styled';

export const FormRow = styled('div')<{ columns?: number }>(
	({ theme, columns = 2 }) => ({
		display: 'grid',
		gridTemplateColumns: `repeat(${columns}, 1fr)`,
		width: '100%',
		gap: theme.spaces.s,
	})
);
