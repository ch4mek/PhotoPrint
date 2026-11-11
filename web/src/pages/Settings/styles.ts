import styled from '@emotion/styled';

import { SettingsPageGridWrapperProps } from './types';

export const SettingsPageGridWrapper = styled(
	'div'
)<SettingsPageGridWrapperProps>(({ theme, isWithHeader }) => ({
	height: '100%',
	paddingTop: isWithHeader ? 0 : theme.spaces.m,
	display: 'flex',
	flexDirection: 'column',
}));
