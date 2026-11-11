import axios, {
	AxiosError,
	AxiosResponse,
	InternalAxiosRequestConfig,
} from 'axios';
import { useLayoutEffect } from 'react';

import { LogoutReasons } from '@/constants/authConstants';
import { HttpStatuses } from '@/constants/httpStatuses';
import { API_URL } from '@/constants/queryPaths';
import { ErrorResponseData } from '@/types/common';

axios.defaults.baseURL = API_URL;

export const useAxiosInterCeptors = (
	token: string | null,
	resetAuthData: (reason?: LogoutReasons, message?: string) => void
) => {
	useLayoutEffect(() => {
		const interceptors = {
			request: (config: InternalAxiosRequestConfig) => {
				const newConfig = { ...config };

				if (!token) {
					resetAuthData();
				}
				newConfig.headers.Authorization = `Bearer ${token}`;

				return newConfig;
			},
			error: (error: AxiosError<ErrorResponseData>) => {
				const errorMessage = error?.response?.data?.message;

				if (
					error?.response?.status === HttpStatuses.Unauthorized &&
					token &&
					errorMessage
				) {
					resetAuthData(LogoutReasons.VIA_MESSAGE, errorMessage);
				}

				if (error?.response?.status === HttpStatuses.Unauthorized && token) {
					resetAuthData();
				}

				return Promise.reject(error);
			},
			response: (response: AxiosResponse) => response,
		};

		const { request, response } = axios.interceptors;
		const reqInterceptor = request.use(interceptors.request, (error) =>
			Promise.reject(error)
		);

		const resInterceptor = response.use((res) => res, interceptors.error);

		return () => {
			request.eject(reqInterceptor);
			response.eject(resInterceptor);
		};
	}, [token]);
};
