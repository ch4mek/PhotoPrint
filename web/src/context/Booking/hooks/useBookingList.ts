import { useMemo } from 'react';

import { isSysAdmin } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { UseBookingListProps } from '@/context/Booking/types';
import { useGetBookings } from '@/queries/booking';
import { useGetRoomsBookings } from '@/queries/booking/useGetRoomsBookings';

export const useBookingList = ({ office }: UseBookingListProps) => {
	const { id, role } = useAuth();

	const { data: desks, isFetching: areDesksFetching } = useGetBookings(
		{
			userId: id,
			includeEmployee: !isSysAdmin(role),
			officeId: office.id,
		},
		{ refetchOnWindowFocus: true }
	);

	const { data: rooms, isFetching: areRoomsFetching } = useGetRoomsBookings(
		office.id,
		{
			initialData: [],
			refetchOnWindowFocus: true,
		}
	);

	const bookingsState = useMemo(
		() => ({
			desks,
			rooms,
		}),
		[desks, rooms]
	);

	return {
		isBookingsDataLoading: areRoomsFetching || areDesksFetching,
		bookingsState,
	};
};
