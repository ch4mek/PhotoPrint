import styled from '@emotion/styled';

export const FieldWithDescriptionWrapper = styled('div')<{ gapSize: number }>(
	({ theme, gapSize }) => ({
		display: 'flex',
		flexDirection: 'column',
		gap: gapSize || theme.spaces.s,
	})
);
