create table server.users (
    id			int				auto_increment,
    username	char (20)		NOT NULL,
    nickname	nchar (20)		NOT NULL,
    email		nchar (50)		NOT NULL,
    type		int				default 0,
    pwd			varbinary (256)	NOT NULL,
    salt		varbinary (128)	NOT NULL,
    avatar		nchar (255)		default "http://i.imgur.com/iiM2hyO.png",
    nick_color	char (10)		default "#FFD3D3D3",
    primary key (id asc),
	unique (email),
    unique (username)
);

