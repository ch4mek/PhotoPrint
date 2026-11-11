import { useTheme } from '@emotion/react';
import { Carousel } from '@mms/mms-ui-library/components/Carousel';
import { EmptyAbsencesDarkIcon } from '@mms/mms-ui-library/icons/base/EmptyAbsencesDarkIcon';
import { EmptyAbsencesLightIcon } from '@mms/mms-ui-library/icons/base/EmptyAbsencesLightIcon';
import { useLayoutEffect, useMemo, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { EmptyResult } from '../EmptyResult';

import { Loader } from '@/components/Loader';
import { UserAbsenceCard } from '@/components/UserAbsenceCard';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { Widget } from '@/components/Widget';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { useResize } from '@/hooks/useResize';
import { useGetUserAbsences } from '@/queries/absences/useGetUserAbsences';
import { AbsenceType } from '@/types/absences';

import {
	ABSENCES_WIDGET_HEIGHT,
	ABSENCES_WIDGET_WITH_CAROUSEL,
	EMPTY_STATE_ICON_HEIGHT,
	getCarouselSlideStyles,
	getWidgetContentStyles,
	MAX_SINGLE_ITEM_WIDTH,
	MAX_SLIDES_TO_SHOW,
} from './constants';
import { AbsencesWrapper, UserAbsenceItemWrapper } from './styles';

export function AbsencesWidget() {
	const theme = useTheme();
	const { t } = useTranslation('home', { keyPrefix: 'widgets.absences' });
	const [windowWidth] = useResize();
	const carouselRef = useRef<HTMLDivElement>(null);
	const [sliderWidth, setSliderWidth] = useState(0);

	const navigate = useCustomNavigate();

	const { id: userId } = useAuth();

	const { data = [], isFetching: isLoading } = useGetUserAbsences(userId);

	useLayoutEffect(() => {
		if (carouselRef.current && !isLoading) {
			setSliderWidth(carouselRef.current.offsetWidth);
		}
	}, [windowWidth, isLoading]);

	const absencesItems = useMemo(
		() =>
			data.filter(
				(absence) =>
					absence.type === AbsenceType.SickDay ||
					absence.type === AbsenceType.Vacation
			),
		[data]
	);

	const handleRedirectToAbsencesTab = () => {
		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: UserProfileTabsId.ABSENCES,
			},
		});
	};

	const needsCarousel = absencesItems.length > MAX_SLIDES_TO_SHOW;
	const isSingle = absencesItems.length === 1;
	const isEmpty = absencesItems.length === 0;

	const absencesList = absencesItems.map((item) => (
		<UserAbsenceItemWrapper
			key={item.inquiryTemplateId}
			maxWidth={isSingle ? MAX_SINGLE_ITEM_WIDTH : undefined}
		>
			<UserAbsenceCard absenceItem={item} width="100%" />
		</UserAbsenceItemWrapper>
	));
	const renderContent = () => {
		if (isEmpty) {
			return (
				<EmptyResult
					darkThemeIcon={
						<EmptyAbsencesDarkIcon height={EMPTY_STATE_ICON_HEIGHT} />
					}
					lightThemeIcon={
						<EmptyAbsencesLightIcon height={EMPTY_STATE_ICON_HEIGHT} />
					}
					title={t('noAbsences')}
				/>
			);
		}

		if (needsCarousel) {
			return (
				<Carousel
					sliderWidth={sliderWidth}
					slidesToShow={MAX_SLIDES_TO_SHOW}
					slideStyles={getCarouselSlideStyles(theme)}
				>
					{absencesList}
				</Carousel>
			);
		}

		return absencesList;
	};

	return (
		<Widget
			{...{ title: t('title') }}
			height={
				needsCarousel ? ABSENCES_WIDGET_WITH_CAROUSEL : ABSENCES_WIDGET_HEIGHT
			}
			widgetContentStyles={
				needsCarousel ? getWidgetContentStyles(theme) : undefined
			}
			handleClick={isEmpty ? undefined : handleRedirectToAbsencesTab}
		>
			<AbsencesWrapper ref={carouselRef}>
				<Loader size="small" {...{ isLoading }}>
					{renderContent()}
				</Loader>
			</AbsencesWrapper>
		</Widget>
	);
}
