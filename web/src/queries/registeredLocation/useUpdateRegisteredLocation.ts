import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	NotificationType,
	REGISTERD_LOCATION_UPDATED,
} from '@/constants/notifications';
import { getRegisteredLocationEndPointUrl } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { UpdateRegisteredLocation } from '@/forms/TeamboardForm/RegisteredLocation/types';
import { createToast } from '@/helpers/createToast';
import { RegisteredLocationResponse } from '@/types/registeredLocation';

export const useUpdateRegisteredLocation = (
	options: UseMutationOptions<
		RegisteredLocationResponse,
		AxiosError,
		UpdateRegisteredLocation
	>,
	toast: ToastContextValues
) =>
	useMutation<RegisteredLocationResponse, AxiosError, UpdateRegisteredLocation>(
		{
			...options,
			mutationFn: ({ currentLocation, registeredLocation, location, userId }) =>
				axios
					.patch(getRegisteredLocationEndPointUrl(String(userId)), {
						currentLocation,
						registeredLocationId: registeredLocation,
						location,
					})
					.then((res) => {
						toast.open(
							createToast(NotificationType.SUCCESS, REGISTERD_LOCATION_UPDATED)
						);

						return res.data;
					})
					.catch((error) => {
						if (error?.response?.data?.Message) {
							toast.open(
								createToast(NotificationType.ERROR, error.response.data.Message)
							);
						}
					}),
		}
	);
