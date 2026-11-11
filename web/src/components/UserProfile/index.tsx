import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useQueryClient } from '@tanstack/react-query';
import { SetStateAction, useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { Loader } from '../Loader';
import { PersonalInfo } from '../PersonalInfo';
import { ProfileAvatar } from '../ProfileAvatar';
import { TabPanel } from '../TabPanel';
import { UpcomingReview } from '../UpcomingReview';

import { isChiefRole } from '@/constants/roles';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth, useToast } from '@/context/index';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { setAuthState } from '@/helpers/localStorage';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useResize } from '@/hooks/useResize';
import { useTabs } from '@/hooks/useTabs';
import { usePersonalData } from '@/queries/personalData/usePersonalData';
import { useUpdateCurrentUser } from '@/queries/user/useUpdateCurrentUser';
import { updateUserCache } from '@/queries/utils/updateUserCache';

import { UserProfileTabs } from './components/UserProfileTabs';
import { tabs } from './constansts';
import { PersonalDataContext } from './context/personalDataContext';
import { TabContext } from './context/tabContext';
import { ContentForm, UserDataWrapper } from './styles';
import {
	PersonalDataContextValues,
	UserProfileProps,
	UserProfileTabsId,
} from './types';
import { getPopUpStylesConfig } from './utils';

export function UserProfile({ handleClose }: UserProfileProps) {
	const theme = useTheme();
	const { t } = useTranslation('profile');
	const [, windowHeight] = useResize();
	const [activeTab, setActiveTab] = useTabs(
		UserProfileTabsId.SUMMARY,
		USER_PROFILE_TAB_ID_PARAM
	);

	const toast = useToast();
	const userInfo = useAuth();
	const { id: userId, role, rightPosition, claims } = userInfo;

	const queryClient = useQueryClient();

	const { mutate: updateCurrentUserMutation } = useUpdateCurrentUser(
		{
			onSuccess: async (dataUser) => {
				setAuthState(dataUser);

				await updateUserCache(queryClient, dataUser);
			},
			onError: createOnErrorHandler(toast),
		},
		toast
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const { data, isFetching, refetch, isLoading } = usePersonalData(userId, {
		onError: createOnErrorHandler(toast),
	});

	const personalDataContextValue = useMemo(
		() =>
			({
				data: data || {},
				isFetching,
				isLoading,
				showLoader: !data,
				refetch,
				userId,
			} as PersonalDataContextValues),
		[data, isFetching, refetch, isLoading, userId]
	);

	const {
		upcomingPerfomanceReview,
		performanceReviewDateStatus,
		primarySpecialization,
		fullName,
		seniority,
	} = personalDataContextValue.data;

	const handleTabChange = useCallback((newTab: SetStateAction<number>) => {
		setActiveTab(newTab);
	}, []);

	const tabContextValue = useMemo(
		() => ({
			handleTabChange,
		}),
		[handleTabChange]
	);

	const tabsToShow = useMemo(
		() =>
			tabs.filter(
				(tab) =>
					!tab.requiredClaims || checkIsHasClaims(tab.requiredClaims, claims)
			),
		[claims]
	);

	return (
		<PopUp
			zIndex={zIndexes.profilePopup}
			title={t('userProfileTitle')}
			onClose={handleClose}
			stylesConfig={popUpStylesConfig}
			type="info"
		>
			<PersonalDataContext.Provider value={personalDataContextValue}>
				<TabContext.Provider value={tabContextValue}>
					<Loader isLoading={isLoading || isFetching} size="small">
						<ContentForm>
							<UserDataWrapper>
								<UpcomingReview
									isFetching={isFetching}
									isLoading={isLoading}
									upcomingPerfomanceReview={upcomingPerfomanceReview}
									performanceReviewDateStatus={performanceReviewDateStatus}
								/>

								<PersonalInfo
									AvatarComponent={ProfileAvatar}
									avatarProps={{
										onSubmit: updateCurrentUserMutation,
										initialValues: userInfo,
									}}
									isShowLoader={isLoading || isFetching}
									seniority={seniority}
									primarySpecialization={primarySpecialization}
									fullName={fullName}
									isShowComments={false}
									refetch={refetch}
									position={rightPosition}
									comments={null}
									isShowSpecialization={!isChiefRole(role)}
								/>
							</UserDataWrapper>

							<UserProfileTabs activeTab={activeTab} onChange={handleTabChange}>
								{tabsToShow.map(({ id: tabId, Component }) => (
									<TabPanel key={tabId} index={tabId} value={activeTab}>
										<Component />
									</TabPanel>
								))}
							</UserProfileTabs>
						</ContentForm>
					</Loader>
				</TabContext.Provider>
			</PersonalDataContext.Provider>
		</PopUp>
	);
}
