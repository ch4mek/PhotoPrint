import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { PropsWithChildren } from 'react';
import { useTranslation } from 'react-i18next';

import { InfoTooltipCircle } from '../InfoTooltipCircle';

import {
	WidgetActionTitle,
	WidgetContentWrapper,
	WidgetHeader,
	WidgetHintWrapper,
	WidgetScrollableContent,
	WidgetTitleWrapper,
	WidgetWrapper,
} from './styles';
import { WidgetProps } from './types';

export function Widget({
	children,
	title,
	titleIcon,
	tooltipContent,
	handleClick,
	height,
	maxHeight,
	widgetContentStyles,
}: PropsWithChildren<WidgetProps>) {
	const theme = useTheme();
	const { t } = useTranslation('common');

	const titleContent = (
		<WidgetTitleWrapper>
			{titleIcon}
			<Typography variant="m-700" color={theme.palette['text-black']}>
				{title}
			</Typography>
		</WidgetTitleWrapper>
	);

	return (
		<WidgetWrapper {...{ height, maxHeight }}>
			<WidgetHeader>
				{tooltipContent ? (
					<WidgetHintWrapper>
						{titleContent}
						<InfoTooltipCircle tooltipContent={tooltipContent} />
					</WidgetHintWrapper>
				) : (
					titleContent
				)}
				{!!handleClick && (
					<WidgetActionTitle onClick={handleClick}>
						<Typography variant="m-600">{t('seeAll')}</Typography>
					</WidgetActionTitle>
				)}
			</WidgetHeader>
			<WidgetContentWrapper>
				<WidgetScrollableContent style={widgetContentStyles}>
					{children}
				</WidgetScrollableContent>
			</WidgetContentWrapper>
		</WidgetWrapper>
	);
}
