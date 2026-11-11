import { useMemo } from 'react';

import { useGetFile } from '@/queries/useGetFile';
import { ImageFileSize } from '@/types/file';

interface UseAvatarOptions {
	enabled?: boolean;
}

export function useAvatar(
	fileId: string | null | undefined,
	options?: UseAvatarOptions
) {
	const { data, isFetching, ...rest } = useGetFile(
		{
			fileId: fileId ?? undefined,
			imageSize: ImageFileSize.Medium,
		},
		{
			enabled: options?.enabled !== false && Boolean(fileId),
		}
	);

	const imgSource = useMemo(
		() => (data ? `data:image/jpeg;base64,${data}` : undefined),
		[data]
	);

	return {
		imgSource: isFetching ? undefined : imgSource,
		isFetching,
		...rest,
	};
}
