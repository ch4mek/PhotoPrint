import { createContext, PropsWithChildren, useContext, useMemo } from 'react';
import { Id, toast } from 'react-toastify';

import { ToastContextValues } from './types';

const ToastContext = createContext({} as ToastContextValues);

export function ToastProvider({ children }: PropsWithChildren) {
	const contextValue = useMemo<ToastContextValues>(
		() => ({
			open: ({ message, settings }) => {
				const { type, ...rest } = settings;
				toast(message, {
					...rest,
					style: {
						backgroundColor: 'transparent',
					},
				});
			},
			subscribe: () => {
				const toastes: Id[] = [];
				toast.onChange((payload) => {
					if (payload.status === 'added') {
						toastes.push(payload.id);
					}

					if (payload.status === 'removed') {
						toastes.shift();
					}

					if (toastes.length > 3) {
						toast.dismiss(toastes[0]);
					}
				});
			},
		}),
		[]
	);

	return <ToastContext.Provider value={contextValue} children={children} />;
}

export const useToast = () => useContext(ToastContext);
