import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { AppLinkWrapper, LinkIconContainer } from './styles';
import { AppLinkProps } from './types';
import { getAppLinkIconColor } from './utils/getAppLinkIconColor';

export function AppLink({
	title,
	icon,
	hasSpecificColor,
	...restProps
}: AppLinkProps) {
	const theme = useTheme();

	return (
		<AppLinkWrapper target="_blank" {...restProps}>
			<LinkIconContainer color={getAppLinkIconColor(hasSpecificColor, theme)}>
				{icon}
			</LinkIconContainer>
			<Typography variant="xs-400" noWrap align="center">
				{title}
			</Typography>
		</AppLinkWrapper>
	);
}
