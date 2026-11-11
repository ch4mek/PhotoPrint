import { PayloadFormValues } from '@/forms/PayloadForm/types';
import { generateRequestBodyObject } from '@/forms/PayloadForm/utils/generateRequestBodyObject';
import { useGetPayloadByUser } from '@/queries/payload/useGetPayloadByUser';
import { UserDetailInfo } from '@/queries/types';
import { useUpdateEmployeePayload } from '@/queries/useUpdateEmpoyeePayload';

export const usePayloadFormMutations = (userInfo: UserDetailInfo) => {
	const { id, year } = userInfo;

	const {
		mutateAsync: mutateUpdateEmployeePayload,
		isPending: isLoadingUpdatePayload,
	} = useUpdateEmployeePayload<PayloadFormValues>(id);

	const {
		data,
		isLoading: isLoadingGetPayloadByUser,
		isFetching,
	} = useGetPayloadByUser(year, generateRequestBodyObject(id));

	const isLoaderVisible =
		isLoadingUpdatePayload || isLoadingGetPayloadByUser || isFetching;

	return {
		payloadData: data,
		isLoaderVisible,
		mutateUpdateEmployeePayload,
	};
};
