export const triggerFileDownload = (data: string | Blob, fileName: string) => {
	const isBlob = data instanceof Blob;

	const url = isBlob ? URL.createObjectURL(data) : data;

	const link = document.createElement('a');
	link.href = url;
	link.setAttribute('download', fileName);
	link.style.display = 'none';

	document.body.appendChild(link);
	link.click();

	link.remove();

	if (isBlob) {
		URL.revokeObjectURL(url);
	}
};
