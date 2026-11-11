import { Button } from '@mms/mms-ui-library/components/Button';
import { useTranslation } from 'react-i18next';

import { PageTitleTypography } from '@/components/PageTitleTypography';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';

import { titleStylesConfig } from './constants';
import { useTimeDependentTitle } from './hooks/useTimeDependentTitle';
import {
	HomeHeaderWrapper,
	PageTitleContent,
	PageTitleWrapper,
} from './styles';

export function HomeHeader() {
	const { title, icon: DayTimeIcon } = useTimeDependentTitle();
	const { t } = useTranslation('home');
	const navigate = useCustomNavigate();

	const handleUserProfileOpen = () =>
		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: UserProfileTabsId.SUMMARY,
			},
		});

	return (
		<HomeHeaderWrapper>
			<PageTitleWrapper>
				<PageTitleContent>
					<PageTitleTypography
						{...{ title }}
						stylesConfig={titleStylesConfig}
					/>
					<DayTimeIcon />
				</PageTitleContent>
			</PageTitleWrapper>
			<Button
				color="secondary"
				isFullWidth={false}
				onClick={handleUserProfileOpen}
			>
				{t('goToProfile')}
			</Button>
		</HomeHeaderWrapper>
	);
}
