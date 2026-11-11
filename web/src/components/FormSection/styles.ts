import styled from '@emotion/styled';

export const FormSectionWrapper = styled('div')<{ gap?: number }>(
	({ theme, gap }) => ({
		display: 'flex',
		flexDirection: 'column',
		gap: gap || theme.spaces.m,
	})
);
