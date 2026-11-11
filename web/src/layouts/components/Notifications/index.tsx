import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { useCallback, useEffect, useRef, useState } from 'react';

import { HttpStatuses } from '@/constants/httpStatuses';
import { API_URL } from '@/constants/queryPaths';
import { useAuth } from '@/context/AuthContext';
import { useGetUserNotificationsPreview } from '@/queries/notifications/useGetNotificationsPreview';
import { NotificationType, UserNotification } from '@/types/notifications';

import {
	defaultReconnectMilliseconds,
	HEADER_INCLUDED_NOTIFCATION_TYPES,
	POPPER_BOTTOM_OFFSET,
	popperOffset,
	popperStyles,
} from './constants';
import { NotificationsButton } from './NotificationButton';
import { NotificationList } from './NotificationsList';
import { notificationsHubEvents } from './NotificationsList/constants';

export function Notifications() {
	const { id: userId } = useAuth();
	const [notificationsAvailableHeight, setNotificationsAvailableHeight] =
		useState<number>(0);
	const [isOpen, setIsOpen] = useState(false);
	const [hasUnreadNotifications, setHasUnreadNotifications] = useState(false);
	const buttonRef = useRef<HTMLButtonElement | null>(null);
	const [connection, setConnection] = useState<null | HubConnection>(null);

	const { token, onLogout } = useAuth();

	const { data, refetch, dataUpdatedAt } = useGetUserNotificationsPreview(
		userId,
		HEADER_INCLUDED_NOTIFCATION_TYPES
	);

	const refetchHasUnreadNotifications = useCallback(() => {
		refetch();
	}, []);

	const handleReceiveNotification = useCallback(
		(notificationData: UserNotification) => {
			if (
				notificationData.notificationType ===
				NotificationType.EventRecordsRemoved
			) {
				refetchHasUnreadNotifications();

				return;
			}

			setHasUnreadNotifications(true);
		},
		[refetchHasUnreadNotifications]
	);

	useEffect(() => {
		if (connection && !isOpen) {
			connection.on(
				notificationsHubEvents.receiveNotification,
				handleReceiveNotification
			);

			return () => {
				connection.off(
					notificationsHubEvents.receiveNotification,
					handleReceiveNotification
				);
			};
		}
	}, [isOpen, connection]);

	useEffect(() => {
		if (token) {
			const newConnection = new HubConnectionBuilder()
				.withUrl(`${API_URL}/hubs/notifications`, {
					withCredentials: false,
					accessTokenFactory: () => token,
				})
				.withAutomaticReconnect({
					nextRetryDelayInMilliseconds: (retryContext) => {
						if (
							retryContext?.retryReason?.message?.includes(
								String(HttpStatuses.Unauthorized)
							)
						) {
							onLogout();

							return null;
						}

						return (
							defaultReconnectMilliseconds[retryContext.previousRetryCount] ??
							null
						);
					},
				})
				.build();

			newConnection.start();

			setConnection(newConnection);

			return () => {
				if (newConnection.connectionId) {
					newConnection.stop();
					setConnection(null);
				}
			};
		}
	}, [token]);

	useEffect(() => {
		if (data !== undefined) {
			setHasUnreadNotifications(data);
		}
	}, [data, dataUpdatedAt]);

	const handleNotificationsButtonClick = useCallback(() => {
		setIsOpen((prevIsOpen) => !prevIsOpen);
	}, []);

	const handleClickOutside = useCallback(() => {
		setIsOpen(false);
	}, []);

	useOutsideClickMany([buttonRef], handleClickOutside);

	const setNotificationsMaxHeight = useCallback((availableHeight: number) => {
		setNotificationsAvailableHeight(availableHeight - POPPER_BOTTOM_OFFSET);
	}, []);

	return (
		<>
			<NotificationsButton
				showBadge={hasUnreadNotifications}
				ref={buttonRef}
				onClick={handleNotificationsButtonClick}
			/>
			{isOpen && (
				<Popover
					open={isOpen}
					anchorElement={buttonRef.current}
					position="bottom-end"
					offset={popperOffset}
					preventOverflow={false}
					popperStyles={popperStyles}
					availableHeight
					setAvailableHeight={setNotificationsMaxHeight}
				>
					<NotificationList
						maxHeight={notificationsAvailableHeight}
						connection={connection}
						refetchUnreadNotificationStatus={refetchHasUnreadNotifications}
						hasUnreadNotifications={hasUnreadNotifications}
						handleNotificationsModalClose={handleNotificationsButtonClick}
						handleReceiveNotification={handleReceiveNotification}
					/>
				</Popover>
			)}
		</>
	);
}
