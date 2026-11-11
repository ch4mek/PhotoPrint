import { Typography } from '@mms/mms-ui-library/components/Typography';
import React, {
	MouseEvent,
	useCallback,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';

import { HEADER_INCLUDED_NOTIFCATION_TYPES } from '../constants';
import {
	isEventRecordsRemovedNotification,
	parseRawNotification,
} from '../utils';

import { Loader } from '@/components/Loader';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useScrollToBottom } from '@/hooks/useScrollToBottom';
import { GetNotificationsParams } from '@/queries/notifications/types';
import { useDeleteAllNotification } from '@/queries/notifications/useDeleteAllNotification';
import { useDeleteNotification } from '@/queries/notifications/useDeleteNotification';
import { useGetUserNotifications } from '@/queries/notifications/useGetNotifications';
import { useReadNotifications } from '@/queries/notifications/useReadNotifications';
import {
	NotificationType,
	RawUserNotification,
	UserNotification,
} from '@/types/notifications';

import { ApproveClearAllDialog } from './ApproveClearAllDialog';
import {
	NOTIFICATION_LIST_PARENT_CLASS,
	NOTIFICATIONS_PER_PAGE,
	notificationsHubEvents,
} from './constants';
import { NotificationsParameteresContext } from './context/NotificationsContext';
import { NoNotificationsPlaceholder } from './NoNotificationsPlaceholder';
import { NotificationListItem } from './NotificationListItem';
import {
	AllClearDialogWrapper,
	ClearAllButton,
	HeaderTypography,
	LOADER_HEIGHT,
	NotificationListWrapper,
	NotificationsListBodySpaceContainer,
	NotificationsListHeaderWrapper,
	ScrollbarWrapper,
} from './styles';
import {
	NotificationListProps,
	NotificationParameteresContextValue,
} from './types';
import { isNotificationReadable, parseNotificationResponse } from './utils';

function NotificationListInnner({
	connection,
	refetchUnreadNotificationStatus,
	handleNotificationsModalClose,
	hasUnreadNotifications,
	maxHeight,
	handleReceiveNotification,
}: NotificationListProps) {
	const { id: userId } = useAuth();
	const listRef = useRef<HTMLDivElement | null>(null);
	const prevFetchDateRef = useRef<number | undefined>(undefined);
	const [notifications, setNotifications] = useState<Array<UserNotification>>(
		[]
	);
	const [isClearAllDialogOpen, setIsClearAllDialogOpen] = useState(false);
	const highestNotificationIdRef = useRef(0);

	const [queryParams, setQueryParams] = useState<GetNotificationsParams>({
		userId,
		skip: notifications.length,
		top: NOTIFICATIONS_PER_PAGE,
		orderBy: `id ${SortDirection.DESC}`,
		filter: {
			isDeleted: false,
			not: {
				notificationType: {
					in: [
						NotificationType.WorkplaceBookingCancelled,
						NotificationType.MeetingRoomBookingCancelled,
					],
				},
			},
		},
	});

	const { data, dataUpdatedAt, isLoading } =
		useGetUserNotifications(queryParams);

	const hasNextNotifications = useMemo(
		() => !data || data.total > notifications.length,
		[data, notifications.length]
	);

	const { mutateAsync: readNotificationsMutationAsync } =
		useReadNotifications();

	useEffect(() => {
		if (data && prevFetchDateRef.current !== dataUpdatedAt) {
			prevFetchDateRef.current = dataUpdatedAt;

			const newNotifications = parseNotificationResponse(data.items);

			if (newNotifications.length) {
				const topNotification = newNotifications[0];

				if (highestNotificationIdRef.current < topNotification.id) {
					highestNotificationIdRef.current = topNotification.id;
				}
			}

			const toReadNotificationIds: number[] = newNotifications
				.filter(isNotificationReadable)
				.map((notification) => notification.id);

			setNotifications((prevItems) => [...prevItems, ...newNotifications]);

			if (toReadNotificationIds.length) {
				readNotificationsMutationAsync(toReadNotificationIds).then(() => {
					refetchUnreadNotificationStatus();
				});
			} else {
				refetchUnreadNotificationStatus();
			}
		}
	}, [data]);

	const handleRemoveDevPlansNotifications = (devPlanId: number | null) => {
		if (devPlanId === null) {
			return;
		}

		setNotifications((prevState) =>
			prevState.filter((notification) => {
				if ('DevelopmentPlanId' in notification) {
					return notification.DevelopmentPlanId !== devPlanId;
				}

				return true;
			})
		);
	};

	useEffect(() => {
		if (connection && !isLoading) {
			const handleReceiveNewNotification = (
				rawNotification: RawUserNotification
			) => {
				if (
					highestNotificationIdRef.current >= rawNotification.id &&
					!isEventRecordsRemovedNotification(rawNotification)
				) {
					return;
				}

				const parsedNotification = parseRawNotification(rawNotification);

				if (isEventRecordsRemovedNotification(parsedNotification)) {
					handleRemoveDevPlansNotifications(
						parsedNotification?.DevelopmentPlanId || null
					);
				} else {
					setNotifications((prevNotifications) => [
						parsedNotification,
						...prevNotifications,
					]);
				}

				if (isNotificationReadable(parsedNotification)) {
					readNotificationsMutationAsync([parsedNotification.id]).then(() => {
						refetchUnreadNotificationStatus();
					});
				}

				handleReceiveNotification(parsedNotification);
			};

			connection.on(
				notificationsHubEvents.receiveNotification,
				handleReceiveNewNotification
			);

			return () => {
				connection.off(
					notificationsHubEvents.receiveNotification,
					handleReceiveNewNotification
				);
			};
		}
	}, [connection, refetchUnreadNotificationStatus, isLoading]);

	const handleNotificationListScrollToBottom = useCallback(() => {
		if (hasNextNotifications) {
			setQueryParams((prevQueryParams) => ({
				...prevQueryParams,
				skip: notifications.length,
			}));
		}
	}, [notifications.length, hasNextNotifications]);

	useScrollToBottom(
		listRef.current,
		handleNotificationListScrollToBottom,
		undefined,
		true
	);

	const { mutateAsync: deleteNotificationAsync } = useDeleteNotification();

	const { mutate: deleteAllNotifications } = useDeleteAllNotification(
		HEADER_INCLUDED_NOTIFCATION_TYPES,
		{
			onSuccess: () => {
				setNotifications([]);
				refetchUnreadNotificationStatus();
			},
		}
	);

	const deleteNotification = useCallback(
		(notificationId: number) => {
			deleteNotificationAsync(notificationId).then(() => {
				setNotifications((prevNotifications) =>
					prevNotifications.filter(({ id }) => notificationId !== id)
				);
				refetchUnreadNotificationStatus();
			});
		},
		[deleteNotificationAsync, refetchUnreadNotificationStatus]
	);

	const readNotification = useCallback((notificationId: number) => {
		readNotificationsMutationAsync([notificationId]).then(() => {
			setNotifications((prevNotifications) =>
				prevNotifications.map((prevNotification) => {
					if (prevNotification.id === notificationId) {
						return {
							...prevNotification,
							read: true,
						};
					}

					return prevNotification;
				})
			);
			refetchUnreadNotificationStatus();
		});
	}, []);

	const notificationParameteresContextValue: NotificationParameteresContextValue =
		useMemo(
			() => ({
				deleteNotification,
				readNotification,
				handleNotificationsModalClose,
			}),
			[deleteNotification, readNotification]
		);

	const clearAllNotifications = useCallback(() => {
		deleteAllNotifications(userId);
	}, [userId]);

	const openClearAllDialog = useCallback(() => {
		setIsClearAllDialogOpen(true);
	}, []);

	const closeClearAllDialog = useCallback(() => {
		setIsClearAllDialogOpen(false);
	}, []);

	const handleApproveClearAllDialog = useCallback(() => {
		clearAllNotifications();
		closeClearAllDialog();
	}, []);

	const handleClearButtonClick = useCallback(() => {
		if (!notifications.length) {
			return;
		}

		if (hasUnreadNotifications) {
			openClearAllDialog();
		} else {
			clearAllNotifications();
		}
	}, [notifications, hasUnreadNotifications]);

	const handleClearAllDialogMouseDown = useCallback((event: MouseEvent) => {
		event.stopPropagation();
	}, []);

	const showLoader = useMemo(
		() => !notifications.length && isLoading,
		[isLoading, Boolean(notifications.length)]
	);

	return (
		<>
			<NotificationListWrapper
				maxHeight={maxHeight}
				data-testid="notifications-list"
			>
				<NotificationsListHeaderWrapper>
					<HeaderTypography variant="l-600">Notifications</HeaderTypography>
					{notifications.length > 0 && (
						<ClearAllButton onClick={handleClearButtonClick}>
							<Typography variant="m-400">Clear all</Typography>
						</ClearAllButton>
					)}
				</NotificationsListHeaderWrapper>
				<NotificationsParameteresContext.Provider
					value={notificationParameteresContextValue}
				>
					<ScrollbarWrapper ref={listRef}>
						<Loader isLoading={showLoader} height={LOADER_HEIGHT} size="small">
							{Boolean(notifications.length) && (
								<NotificationsListBodySpaceContainer
									className={NOTIFICATION_LIST_PARENT_CLASS}
								>
									{notifications.map((item) => (
										<NotificationListItem key={item.id} item={item} />
									))}
								</NotificationsListBodySpaceContainer>
							)}
							{!notifications.length && <NoNotificationsPlaceholder />}
						</Loader>
					</ScrollbarWrapper>
				</NotificationsParameteresContext.Provider>
			</NotificationListWrapper>
			{isClearAllDialogOpen && (
				<AllClearDialogWrapper onMouseDown={handleClearAllDialogMouseDown}>
					<ApproveClearAllDialog
						onClose={closeClearAllDialog}
						onApprove={handleApproveClearAllDialog}
					/>
				</AllClearDialogWrapper>
			)}
		</>
	);
}

export const NotificationList = React.memo(NotificationListInnner);
