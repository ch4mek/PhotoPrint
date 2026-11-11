import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { EmptyPlansDarkIcon } from '@mms/mms-ui-library/icons/base/EmptyPlansDarkIcon';
import { EmptyPlansLightIcon } from '@mms/mms-ui-library/icons/base/EmptyPlansLightIcon';
import { useTranslation } from 'react-i18next';

import { HOME_MOBILE_BREAKPOINT } from '../../constants';
import { EmptyResult } from '../EmptyResult';

import { Loader } from '@/components/Loader';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { Widget } from '@/components/Widget';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';

import { DevelopmentPlansList } from './components/DevelopmentPlansList';
import { useSendPlanToReview } from './hooks/useSendPlanToReview';
import { DevelopmentPlanWidgetProps } from './types';

const DEVELOPMENT_PLAN_WIDGET_HEIGHT_MOBILE = 238;

export function DevelopmentPlanWidget({
	data,
	isLoading,
}: DevelopmentPlanWidgetProps) {
	const navigate = useCustomNavigate();
	const { t } = useTranslation('home', {
		keyPrefix: 'widgets.developmentPlan',
	});
	const { isPending, handleSendToReview } = useSendPlanToReview();
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);

	const handleRedirectToDevelopmentPlanTab = () => {
		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: UserProfileTabsId.DEVELOPMENT_PLAN,
			},
		});
	};

	return (
		<Widget
			{...{ title: t('title') }}
			height={isDesktopView ? undefined : DEVELOPMENT_PLAN_WIDGET_HEIGHT_MOBILE}
			handleClick={handleRedirectToDevelopmentPlanTab}
		>
			<Loader size="small" isLoading={isLoading || isPending}>
				{data.length === 0 ? (
					<EmptyResult
						darkThemeIcon={<EmptyPlansDarkIcon />}
						lightThemeIcon={<EmptyPlansLightIcon />}
						title={t('noPlan')}
					/>
				) : (
					<DevelopmentPlansList {...{ data, handleSendToReview }} />
				)}
			</Loader>
		</Widget>
	);
}
