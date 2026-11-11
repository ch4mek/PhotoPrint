import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { EmptyBirthdaysDarkIcon } from '@mms/mms-ui-library/icons/base/EmptyBirthdaysDarkIcon';
import { EmptyBirthdaysLightIcon } from '@mms/mms-ui-library/icons/base/EmptyBirthdaysLightIcon';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { EmptyResult } from '../EmptyResult';

import backgroundImageLarge from '@/assets/common/birthdayCardLarge.jpg';
import backgroundImageMedium from '@/assets/common/birthdayCardMedium.jpg';
import backgroundImageSmall from '@/assets/common/birthdayCardSmall.jpg';
import backgroundImageMSmall from '@/assets/common/birthdayCardSmallLong.jpg';
import { Widget } from '@/components/Widget';
import {
	HOME_MOBILE_BREAKPOINT,
	HOME_MOBILE_S_BREAKPOINT,
} from '@/pages/Home/HomeContent/constants';
import { useShowModal } from '@/pages/hooks/useShowModal';

import { AllBirthdaysPopup } from './components/AllBirthdaysPopup';
import { BirthdayCard } from './components/BirthdayCard';
import {
	NUMBER_OF_CARDS_FOR_FULL_VIEW,
	WIDGET_MAX_HEIGHT_FOR_MOBILE,
} from './constants';
import { BirthdaysWidgetWrapper } from './styles';
import { BirthdaysWidgetProps } from './types';

export function BirthdaysWidget({ data }: BirthdaysWidgetProps) {
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);
	const isDesktopSView = useHasScreenWidth(HOME_MOBILE_S_BREAKPOINT);
	const { t } = useTranslation('home', { keyPrefix: 'widgets.birthdays' });

	const {
		isOpen: showAllBirthdaysPopup,
		handleOpen: openAllBirthdaysPopup,
		handleClose: closeAllBirthdaysPopup,
	} = useShowModal();

	const backgroundImage = useMemo(() => {
		if (!isDesktopView) {
			return isDesktopSView ? backgroundImageMSmall : backgroundImageSmall;
		}

		return data.length > NUMBER_OF_CARDS_FOR_FULL_VIEW
			? backgroundImageMedium
			: backgroundImageLarge;
	}, [isDesktopView, data.length, isDesktopSView]);

	return (
		<Widget
			{...{ title: t('title') }}
			height={isDesktopView ? undefined : 'auto'}
			maxHeight={isDesktopView ? undefined : WIDGET_MAX_HEIGHT_FOR_MOBILE}
			handleClick={openAllBirthdaysPopup}
		>
			{data.length === 0 ? (
				<EmptyResult
					darkThemeIcon={<EmptyBirthdaysDarkIcon />}
					lightThemeIcon={<EmptyBirthdaysLightIcon />}
					title={t('noBirthdays')}
				/>
			) : (
				<BirthdaysWidgetWrapper countOfCards={data.length}>
					{data.map((item) => (
						<BirthdayCard
							key={`${item.department}-${item.lastName}-${item.firstName}`}
							{...{ item, backgroundImage }}
						/>
					))}
				</BirthdaysWidgetWrapper>
			)}
			{showAllBirthdaysPopup && (
				<AllBirthdaysPopup closePopup={closeAllBirthdaysPopup} />
			)}
		</Widget>
	);
}
