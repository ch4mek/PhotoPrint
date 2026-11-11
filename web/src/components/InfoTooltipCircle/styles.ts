import styled from '@emotion/styled';
import { InfoCircleIcon as InfoCircleIconElement } from '@mms/mms-ui-library/icons/base/InfoCircleIcon';
import { CSSProperties } from 'react';

const INFO_CIRCLE_ICON_SIZE = 11;

export const InfoCircle = styled('div')<{ stylesConfig?: CSSProperties }>(
	({ stylesConfig }) => ({
		display: 'flex',
		justifyContent: 'space-between',
		alignItems: 'center',

		...stylesConfig,
	})
);

export const InfoCircleIcon = styled(InfoCircleIconElement)(({ theme }) => ({
	color: theme.palette['graphics-scroll'],
	width: INFO_CIRCLE_ICON_SIZE,
	height: INFO_CIRCLE_ICON_SIZE,
}));
