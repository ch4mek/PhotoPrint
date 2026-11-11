import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { Loader } from '@/components/Loader';
import { TabPanel } from '@/components/TabPanel';
import { NotificationType as ToastType } from '@/constants/notifications';
import { SortDirection } from '@/constants/sort';
import { useBookingContext, withBookingProvider } from '@/context/Booking';
import { useAuth, useConfiguration, useToast } from '@/context/index';
import { SelectedBookingItemType } from '@/context/types';
import { isAllowToBook } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import { parseRawNotification } from '@/layouts/components/Notifications/utils';
import { BookingBlock, BookingWorkplace } from '@/pages/Booking/components';
import { BookingMapProvider } from '@/pages/Booking/context/BookingMapContext';
import { useCheckCollisions } from '@/queries/booking';
import { useGetOfficeWorkplaces } from '@/queries/booking/useGetSpaceWorkplaces';
import { useGetUserNotifications } from '@/queries/notifications/useGetNotifications';
import { useReadNotifications } from '@/queries/notifications/useReadNotifications';
import { NotificationType } from '@/types/notifications';

import { BookingWarningPopup } from './components/BookingWarningPopup';
import { BookingCancelledPopupBody } from './components/PopUps/BookingCancelledPopupBody';
import { BOOKING_TABS_CHANGE_SCREEN_WIDTH, bookingTabsKeys } from './constants';
import {
	BookingContentWrapper,
	BookingPageWrapper,
	BookingWrapper,
} from './styles';
import { BookingTabsIndexes } from './types';

function BookingPageInner() {
	const [isAllowBookPopupOpen, setIsAllowBookPopupOpen] = useState(false);
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'modals',
	});
	const { t: tBooking } = useTranslation('booking');

	const bookingTabs = useMemo(
		() =>
			bookingTabsKeys.map((tab) => ({
				...tab,
				text: tBooking(tab.text),
			})),
		[tBooking]
	);

	const hideTabs = useHasScreenWidth(BOOKING_TABS_CHANGE_SCREEN_WIDTH);
	const toast = useToast();
	const { id: userId, role, departmentId, email } = useAuth();

	const {
		firstDay,
		lastDay,
		employee,
		initialEmployee,
		selectedItem,
		weeksCount,
		office,
		isBlockedDesk,
		setHasWorkplaceCollision,
		setOfficeWorkplaces,
		selectedSpaceId,
		activeTabIndex,
		setActiveTabIndex,
	} = useBookingContext();

	const { isLoading: loading, configurableValues } = useConfiguration();

	const { data: officeWorkplacesData, isLoading: isOfficeWorkplacesLoading } =
		useGetOfficeWorkplaces(office.id);

	useEffect(() => {
		if (officeWorkplacesData) {
			setOfficeWorkplaces(officeWorkplacesData);
		}
	}, [officeWorkplacesData]);

	const { isSuccess, data, refetch } = useGetUserNotifications({
		top: 1,
		skip: 0,
		userId,
		orderBy: `id ${SortDirection.DESC}`,
		filter: {
			isDeleted: false,
			read: false,
			notificationType: {
				in: [
					NotificationType.WorkplaceBookingCancelled,
					NotificationType.MeetingRoomBookingCancelled,
				],
			},
		},
	});

	const { mutate: readNotificationsMutation } = useReadNotifications({
		onSuccess: () => {
			toast.open(
				createToast(
					ToastType.SUCCESS,
					tBooking('toasts.notificationMarkedAsRead')
				)
			);
			refetch();
		},
	});

	const handleReadNotification = (notificationId: number) => () => {
		readNotificationsMutation([notificationId]);
	};

	const handleChangeTab = useCallback((newActiveTab: number) => {
		setActiveTabIndex(newActiveTab);
	}, []);

	const notification = useMemo(() => {
		if (!data || !data.items[0]) {
			return null;
		}

		return parseRawNotification(data.items[0]);
	}, [data]);

	const handleDeskCollisionExist = () => {
		setHasWorkplaceCollision(true);
	};

	const handleDeskCollisionAbsent = () => {
		setHasWorkplaceCollision(false);
	};

	useCheckCollisions({
		desk:
			selectedItem?.type === SelectedBookingItemType.Desk ? selectedItem : null,
		fromDate: firstDay,
		toDate: lastDay,
		weeksInterval: weeksCount,
		officeId: office.id,
		userId: employee?.id ?? initialEmployee.id,
		isBlockedDesk,
		onDeskCollisionExist: handleDeskCollisionExist,
		onDeskCollisionAbsent: handleDeskCollisionAbsent,
	});

	const handleAllowPopupClose = () => setIsAllowBookPopupOpen(false);

	useEffect(() => {
		setIsAllowBookPopupOpen(
			!isAllowToBook(role, departmentId, office, selectedSpaceId, email)
		);
	}, [role, departmentId, office, selectedSpaceId, email]);

	const isLoadingVisible =
		loading || !configurableValues.offices || isOfficeWorkplacesLoading;

	return (
		<Loader isLoading={isLoadingVisible}>
			<BookingPageWrapper scroll="both" maxHeight="100%" maxWidth="100%">
				<BookingContentWrapper>
					{!hideTabs && (
						<Tabs
							activeTab={activeTabIndex}
							onChangeTab={handleChangeTab}
							data={bookingTabs}
						/>
					)}
					<BookingMapProvider>
						<BookingWrapper scroll="both" maxHeight="100%" maxWidth="100%">
							{!hideTabs ? (
								<>
									<TabPanel
										value={activeTabIndex}
										index={BookingTabsIndexes.BookingForm}
									>
										<BookingBlock />
									</TabPanel>
									<TabPanel
										value={activeTabIndex}
										index={BookingTabsIndexes.Workplace}
									>
										<BookingWorkplace />
									</TabPanel>
								</>
							) : (
								<>
									<BookingBlock />
									<BookingWorkplace />
								</>
							)}
						</BookingWrapper>
					</BookingMapProvider>
				</BookingContentWrapper>
			</BookingPageWrapper>
			{isSuccess && notification && (
				<PopUp
					title={tCommon('warningTitle')}
					onClose={handleReadNotification(notification.id)}
					type="approve"
					headerType="warning"
					showCloseButton
					key={notification.id}
				>
					<BookingCancelledPopupBody notification={notification} />
				</PopUp>
			)}
			{isAllowBookPopupOpen && (
				<BookingWarningPopup
					warning={tBooking('tooltips.bookingNotAllowed')}
					handleClose={handleAllowPopupClose}
				/>
			)}
		</Loader>
	);
}

export const BookingPage = withBookingProvider(BookingPageInner);
