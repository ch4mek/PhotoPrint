import styled from '@emotion/styled';
import { BigSuccessIcon } from '@mms/mms-ui-library/icons/base/BigSuccessIcon';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';

const ICON_SIZE = 20;

export const NoAccessIcon = styled(CrossIcon)(({ theme }) => ({
	color: theme.palette['graphics-light-gray'],
	height: `${ICON_SIZE}px`,
	width: `${ICON_SIZE}px`,
}));

export const FullAccessIcon = styled(BigSuccessIcon)(({ theme }) => ({
	color: theme.palette['graphics-accent'],
	height: `${ICON_SIZE}px`,
	width: `${ICON_SIZE}px`,
}));

export const PartialAccessIcon = styled(BigSuccessIcon)(({ theme }) => ({
	color: theme.palette['graphics-primary-2'],
	height: `${ICON_SIZE}px`,
	width: `${ICON_SIZE}px`,
}));
