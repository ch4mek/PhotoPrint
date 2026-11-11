import styled from '@emotion/styled';

export const MicrofrontendsPanelPageWrapper = styled('section')(
	({ theme }) => ({
		padding: `${theme.spaces.m}px 0`,
		height: '100%',
		display: 'flex',
		flexDirection: 'column',
	})
);
